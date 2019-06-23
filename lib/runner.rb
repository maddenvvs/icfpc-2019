require 'colorize'
require 'parallel'
require 'concurrent'

class Runner
	INPUT_PATH = File.join(__dir__, '..', 'input')
	OUTPUT_PATH = File.join(__dir__, '..', 'output')
	REPORT_PERIOD = 30

	attr_accessor :cpus

	def initialize from, to
		@processed_good_qty = 0
		@processed_bad_qty = 0
		@from = from
		@to = to
	end

	def start cmd
		Dir.mkdir OUTPUT_PATH unless Dir.exist? OUTPUT_PATH

		cpus_qty = if @cpus < 1
			Concurrent.physical_processor_count
		else
			@cpus
		end

		# status displaying
		Thread.new {
			loop do
				sleep REPORT_PERIOD
				show_stats
			end
		}
		at_exit { show_stats }

		# parallel processing magic
		Parallel.each(@from..@to, in_threads: cpus_qty) do |i|
			prefix = "problem #{i}".colorize(:yellow)
			filename = get_problem_filename i

			input_path = File.join(INPUT_PATH, filename)
			if File.exist? input_path				
				show_info "start #{prefix}"

				start_time = Time.now.to_f

				res = nil
				begin
					# executing problem solving
					IO.popen(cmd, 'w+', :err=> [:child, :out]) do |subprocess|
						subprocess.print(File.read(input_path))
						subprocess.close_write
						res = subprocess.read
					end

					elapsed = (Time.now.to_f - start_time).round(3)

					# result processing
					if $?.exitstatus == 0
						if res.to_s.empty?
							show_error "#{prefix} empty response with good exit code"
							@processed_bad_qty += 1
						else
							output_path = File.join(OUTPUT_PATH, File.basename(filename, '.desc') + '.sol')
							File.write(output_path, res)
							show_output "%s success in %s ms" % [prefix, elapsed.to_s.colorize(:green)]
							@processed_good_qty += 1
						end
					else
						show_error "%s exit code: %s\noutput: %s" % [prefix, $?.exitstatus, res.to_s.colorize(:light_red)]
						@processed_bad_qty += 1
					end
				rescue
					show_error "%s unknown error %s" % [prefix, $!.inspect]
					@processed_bad_qty += 1
				end
			else
				show_error "#{prefix} no problem file #{filename}"
				@processed_bad_qty += 1
			end
		end
	end

	private

	def show_stats
		total = @to - @from + 1
		show_info "processed %s/%s of %s" % [@processed_good_qty.to_s.colorize(:green), @processed_bad_qty.to_s.colorize(:light_red), total]
	end

	def show_output msg
		puts msg
	end

	def show_info msg
		STDERR.puts "[info] " + msg.to_s
	end

	def show_error msg
		STDERR.puts "[error] ".colorize(:red) + msg.to_s
	end

	def get_problem_filename num
		"prob-%s.desc" % num.to_s.rjust(3, '0')
	end
end