require 'colorize'
require 'parallel'
require 'concurrent'

class Runner
	INPUT_PATH = File.join(__dir__, '..', 'input')
	OUTPUT_PATH = File.join(__dir__, '..', 'output', Time.now.strftime('%Y-%m-%d_%H-%M-%S'))

	attr_accessor :cpus

	def initialize from, to
		@processed_qty = 0
		@from = from
		@to = to
	end

	def start cmd
		Dir.mkdir OUTPUT_PATH unless Dir.exist? OUTPUT_PATH

		at_exit {
			if Dir.empty? OUTPUT_PATH
				puts "nothing processed, removing output dir"
				Dir.unlink(OUTPUT_PATH)
			end
		}

		cpus_qty = if @cpus < 1
			Concurrent.physical_processor_count
		else
			@cpus
		end

		Parallel.each(@from..@to, in_threads: cpus_qty) do |i|
			prefix = "problem #{i}".colorize(:yellow)
			filename = get_problem_filename i

			input_path = File.join(INPUT_PATH, filename)
			if File.exist? input_path				
				show_info "start #{prefix}"

				start_time = Time.now.to_f

				res = nil
				IO.popen(cmd, 'w+') do |subprocess|
					subprocess.print("hello #{i}\npello #{i}")
					subprocess.close_write
					res = subprocess.read
				end

				elapsed = (Time.now.to_f - start_time).round(3)

				if $?.exitstatus == 0
					if res.to_s.empty?
						show_error "#{prefix} empty response with good exit code"
					else
						output_path = File.join(OUTPUT_PATH, File.basename(filename, '.desc') + '.sol')
						File.write(output_path, res)
						show_output "%s success in %s ms" % [prefix, elapsed.to_s.colorize(:green)]
					end
				else
					show_error "#{prefix} exit code: #{$?.exitstatus}"
				end
			else
				show_error "#{prefix} no problem file #{filename}"
			end
		end
	end

	private

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